#!/bin/sh

ROOT_DIR=/app

# Replace env vars in files served by NGINX
for file in $ROOT_DIR/assets/*.js*;
do
  sed -i 's|__YT_AUDIO_SERVER_PORT__|'${YT_AUDIO_SERVER_PORT}'|g' $file
  # Your other variables here...
done
# Starting NGINX
nginx -g 'daemon off;'