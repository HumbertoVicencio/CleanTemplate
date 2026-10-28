#!/usr/bin/env bash
#
# rename-project.sh — renombra toda la plantilla a un proyecto nuevo.
#
#   ./rename-project.sh GestCond            -> Kcl.Reso.GestCond.*
#   ./rename-project.sh Contoso.Billing     -> Contoso.Billing.*   (nombre con punto = prefijo completo)
#   ./rename-project.sh GestCond --dry-run  -> muestra los cambios sin aplicarlos
#
# Opciones:
#   --dry-run         No modifica nada; lista carpetas, archivos y reemplazos.
#   --projects-only   Solo renombra proyectos/namespaces. Deja intactos el slug de
#                     infraestructura (BD, issuer JWT, contenedores) y el UserSecretsId.
#   -y, --yes         No pide confirmación.
#
# El prefijo actual se detecta solo (a partir del .slnx), así que el script se puede
# volver a ejecutar sobre un proyecto ya renombrado.
#
# Compatible con el bash 3.2 que trae macOS.

set -euo pipefail

cd "$(dirname "$0")"
SELF="$(basename "$0")"

DRY_RUN=0
PROJECTS_ONLY=0
ASSUME_YES=0
NEW_NAME=""

while [ $# -gt 0 ]; do
  case "$1" in
    --dry-run)       DRY_RUN=1 ;;
    --projects-only) PROJECTS_ONLY=1 ;;
    -y|--yes)        ASSUME_YES=1 ;;
    -h|--help)       sed -n '2,18p' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
    -*)              echo "Opción desconocida: $1" >&2; exit 1 ;;
    *)
      if [ -n "$NEW_NAME" ]; then echo "Solo se admite un nombre de proyecto." >&2; exit 1; fi
      NEW_NAME="$1" ;;
  esac
  shift
done

if [ -z "$NEW_NAME" ]; then
  echo "Uso: ./$SELF <NombreProyecto> [--dry-run] [--projects-only] [-y]" >&2
  exit 1
fi

if ! [[ "$NEW_NAME" =~ ^[A-Za-z][A-Za-z0-9]*(\.[A-Za-z][A-Za-z0-9]*)*$ ]]; then
  echo "Nombre inválido: '$NEW_NAME'. Usa PascalCase, opcionalmente con puntos (p. ej. GestCond o Contoso.Billing)." >&2
  exit 1
fi

TMPDIR_RP="$(mktemp -d)"
trap 'rm -rf "$TMPDIR_RP"' EXIT

# ---------------------------------------------------------------------------
# 1. Detectar el estado actual
# ---------------------------------------------------------------------------
find . -maxdepth 1 -name '*.slnx' -not -name '.*' > "$TMPDIR_RP/slnx"
SLN_COUNT="$(wc -l < "$TMPDIR_RP/slnx" | tr -d ' ')"
if [ "$SLN_COUNT" != "1" ]; then
  echo "Se esperaba exactamente un .slnx en la raíz; encontrados: $SLN_COUNT" >&2
  exit 1
fi
OLD_PREFIX="$(basename "$(head -1 "$TMPDIR_RP/slnx")" .slnx)"

# Nombre sin punto -> conserva la organización actual (Kcl.Reso.CleanTemplate -> Kcl.Reso.<Nuevo>)
case "$NEW_NAME" in
  *.*) NEW_PREFIX="$NEW_NAME" ;;
  *)
    ORG="${OLD_PREFIX%.*}"
    if [ "$ORG" = "$OLD_PREFIX" ]; then NEW_PREFIX="$NEW_NAME"; else NEW_PREFIX="$ORG.$NEW_NAME"; fi ;;
esac

if [ "$OLD_PREFIX" = "$NEW_PREFIX" ]; then
  echo "El proyecto ya se llama '$NEW_PREFIX'. Nada que hacer."
  exit 0
fi

# Slug de infraestructura (nombre de BD, issuer JWT, contenedores). Se lee del compose
# para que el script siga funcionando tras renombrados previos.
OLD_SLUG="$(perl -ne 'if (/POSTGRES_DB=(\S+)/) { print $1; exit }' docker-compose.yml || true)"
NEW_SLUG="$(printf '%s' "${NEW_PREFIX##*.}" | perl -pe 's/([a-z0-9])([A-Z])/$1-$2/g' | tr '[:upper:]' '[:lower:]' | tr '.' '-')"

echo "Prefijo:  $OLD_PREFIX  ->  $NEW_PREFIX"
if [ "$PROJECTS_ONLY" -eq 0 ]; then
  echo "Slug:     ${OLD_SLUG:-(no detectado)}  ->  $NEW_SLUG"
  echo "UserSecretsId: se generará uno nuevo"
fi
echo

if [ "$DRY_RUN" -eq 0 ] && [ "$ASSUME_YES" -eq 0 ]; then
  printf '¿Aplicar los cambios? [y/N] '
  read -r answer
  case "$answer" in
    [Yy]*) ;;
    *) echo "Cancelado."; exit 0 ;;
  esac
fi

# Reemplaza una cadena literal en una lista de archivos (uno por línea).
# El paso por NUL es para tolerar rutas con espacios, como "Dockerfile (.NET 8)".
# Uso: replace_in_list <archivo-lista> <viejo> <nuevo> <etiqueta>
replace_in_list() {
  list_file="$1"; old="$2"; new="$3"; label="$4"
  if [ ! -s "$list_file" ]; then return 0; fi
  count="$(grep -c . < "$list_file" || true)"
  if [ "$DRY_RUN" -eq 1 ]; then
    echo
    echo "$label: '$old' -> '$new' en $count archivos:"
    sed 's/^/  /' "$list_file"
  else
    tr '\n' '\0' < "$list_file" |
      OLD="$old" NEW="$new" xargs -0 perl -pi -e 's/\Q$ENV{OLD}\E/$ENV{NEW}/g'
  fi
}

# ---------------------------------------------------------------------------
# 2. Limpiar salidas de compilación (contienen los nombres viejos)
# ---------------------------------------------------------------------------
if [ "$DRY_RUN" -eq 0 ]; then
  find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
fi

# ---------------------------------------------------------------------------
# 3. Renombrar carpetas y archivos (-depth: hijos antes que padres)
# ---------------------------------------------------------------------------
find . -depth -name "*${OLD_PREFIX}*" -not -path './.git/*' > "$TMPDIR_RP/paths"
while IFS= read -r path; do
  [ -z "$path" ] && continue
  base="$(basename "$path")"
  new_base="${base//$OLD_PREFIX/$NEW_PREFIX}"
  new_path="$(dirname "$path")/$new_base"
  if [ "$DRY_RUN" -eq 1 ]; then
    echo "mv  $path  ->  $new_path"
  else
    mv "$path" "$new_path"
  fi
done < "$TMPDIR_RP/paths"

# ---------------------------------------------------------------------------
# 4. Reemplazar el prefijo dentro de los archivos de texto
#    (namespaces, usings, ProjectReference, .slnx, Dockerfiles, compose, CI,
#     snapshots de EF Core y los skills de .claude)
# ---------------------------------------------------------------------------
grep -rlI --fixed-strings \
  --exclude-dir=.git --exclude-dir=bin --exclude-dir=obj --exclude-dir=.containers \
  --exclude="$SELF" "$OLD_PREFIX" . > "$TMPDIR_RP/files" || true
replace_in_list "$TMPDIR_RP/files" "$OLD_PREFIX" "$NEW_PREFIX" "Prefijo"

# ---------------------------------------------------------------------------
# 5. Ajustes por proyecto: slug de infraestructura, contenedores, user secrets
# ---------------------------------------------------------------------------
if [ "$PROJECTS_ONLY" -eq 0 ]; then

  # 5a. Slug: BD de Postgres, cadena de conexión, issuer JWT, Testcontainers,
  #     BD en memoria de los tests y DockerComposeProjectName.
  #     README.md se excluye: la URL 'pragmatic-clean-architecture' contiene el slug.
  if [ -n "$OLD_SLUG" ]; then
    grep -rlI --fixed-strings \
      --exclude-dir=.git --exclude-dir=bin --exclude-dir=obj --exclude-dir=.containers \
      --exclude="$SELF" --exclude=README.md "$OLD_SLUG" . > "$TMPDIR_RP/slugfiles" || true
    replace_in_list "$TMPDIR_RP/slugfiles" "$OLD_SLUG" "$NEW_SLUG" "Slug"
  fi

  # 5b. Nombres de contenedor e imagen, para poder levantar varios proyectos a la vez.
  #     El patrón quita el prefijo anterior antes de poner el nuevo (idempotente).
  if [ "$DRY_RUN" -eq 1 ]; then
    echo
    echo "docker-compose.yml: container_name/image con prefijo '$NEW_SLUG-'"
  else
    SLUG="$NEW_SLUG" PREV_SLUG="${OLD_SLUG:-__none__}" perl -pi -e '
      s/container_name:\s*(?:\Q$ENV{PREV_SLUG}\E-)?(web-api|postgres|seq)\b/container_name: $ENV{SLUG}-$1/g;
      s/(image:\s*\$\{DOCKER_REGISTRY-\})(?:\Q$ENV{PREV_SLUG}\E-)?webapi\b/$1$ENV{SLUG}-webapi/g;
    ' docker-compose.yml
  fi

  # 5c. UserSecretsId nuevo: los secretos no deben compartirse entre proyectos.
  if [ "$DRY_RUN" -eq 1 ]; then
    WEBAPI_CSPROJ="src/$OLD_PREFIX.Web.Api/$OLD_PREFIX.Web.Api.csproj"
  else
    WEBAPI_CSPROJ="src/$NEW_PREFIX.Web.Api/$NEW_PREFIX.Web.Api.csproj"
  fi
  if [ -f "$WEBAPI_CSPROJ" ]; then
    NEW_GUID="$(uuidgen | tr '[:upper:]' '[:lower:]')"
    if [ "$DRY_RUN" -eq 1 ]; then
      echo "$WEBAPI_CSPROJ: UserSecretsId -> $NEW_GUID"
    else
      GUID="$NEW_GUID" perl -pi -e 's{<UserSecretsId>.*?</UserSecretsId>}{<UserSecretsId>$ENV{GUID}</UserSecretsId>}' "$WEBAPI_CSPROJ"
    fi
  fi
fi

# ---------------------------------------------------------------------------
echo
if [ "$DRY_RUN" -eq 1 ]; then
  echo "Dry run: no se modificó nada."
  exit 0
fi

echo "Listo. El proyecto ahora es $NEW_PREFIX."
echo
echo "Siguientes pasos:"
echo "  dotnet build $NEW_PREFIX.slnx"
echo "  dotnet test  $NEW_PREFIX.slnx      # requiere Docker para los tests de integración"
echo
echo "Revisa a mano:"
echo "  - README.md (el texto sigue describiendo la plantilla)"
echo "  - docker-compose.yml: los puertos 5000/5001/5432/8081 siguen fijos;"
echo "    cámbialos si vas a levantar varios proyectos en paralelo."
