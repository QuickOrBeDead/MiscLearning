#!/bin/sh

ROOT_DIR=/app

# Replace env vars in files served by NGINX
for file in $ROOT_DIR/js/*.js*;
do
  sed -i 's|VITE_YT_AUDIO_SERVER|'__${YT_AUDIO_SERVER}__'|g' $file
  # Your other variables here...
done
# Starting NGINX
nginx -g 'daemon off;'