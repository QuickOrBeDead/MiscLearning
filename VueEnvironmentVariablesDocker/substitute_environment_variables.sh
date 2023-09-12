#!/bin/sh

ROOT_DIR=/app

keys="VARIABLE1
VARIABLE2"

# Replace env vars in files served by NGINX
for file in $ROOT_DIR/assets/config-*.js*;
do
  echo "Processing $file ...";
  for key in $keys
  do
    value=$(eval echo \$$key)
    echo "replace $key by $value"
    sed -i 's|__'"$key"'__|'"$value"'|g' $file
  done
done