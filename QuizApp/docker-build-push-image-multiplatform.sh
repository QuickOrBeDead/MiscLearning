docker buildx use mybuilder
docker buildx build --platform linux/amd64,linux/arm64 --push . -t boraakgn/quiz-app