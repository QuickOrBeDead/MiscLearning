FROM node:18-alpine as builder
ADD . /app

ENV NODE_ENV=production
WORKDIR /app
RUN npm ci --omit=dev

FROM node:18-alpine as final
COPY --from=builder /app/node_modules /app/node_modules
COPY --from=builder /app/index.js /app/index.js

WORKDIR /app
EXPOSE 8080
CMD [ "node", "index.js" ]