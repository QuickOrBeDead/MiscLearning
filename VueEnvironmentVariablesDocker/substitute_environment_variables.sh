#!/bin/sh

ROOT_DIR=/app

# Replace env vars in files served by NGINX
for file in $ROOT_DIR/assets/config-*.js*;
do
  sed -i 's|__VARIABLE1__|'${VARIABLE1}'|g' $file
  sed -i 's|__VARIABLE2__|'${VARIABLE2}'|g' $file
done
# Starting NGINX
nginx -g 'daemon off;'