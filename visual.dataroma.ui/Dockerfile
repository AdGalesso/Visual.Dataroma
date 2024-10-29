# Use the official Node.js image
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm i
COPY . .
RUN npm run build

# Use an Nginx image to serve the built app
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]