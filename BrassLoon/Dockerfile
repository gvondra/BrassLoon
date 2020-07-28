### STAGE 1: Build ###
FROM node:12.7-alpine AS build
WORKDIR /usr/src/app
COPY . .
RUN npm install
RUN npm run build --prod=true
### STAGE 2: Run ###
FROM socialengine/nginx-spa
COPY --from=build /usr/src/app/dist/BrassLoon /app