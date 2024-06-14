FROM node:22-alpine as build
EXPOSE 3000

ENV PUBLIC_URL=#{AZURE_GATEWAY_ENDPOINT_FRONTEND}#
ENV REACT_APP_AZURE_GATEWAY_ENDPOINT_BACKEND01=#{AZURE_GATEWAY_ENDPOINT_BACKEND01}#
ENV REACT_APP_AZURE_GATEWAY_ENDPOINT_BACKEND02=#{AZURE_GATEWAY_ENDPOINT_BACKEND02}#

WORKDIR /app
COPY front/package.json .
COPY front/yarn.lock .
RUN yarn install && yarn add react-scripts
COPY front/. .
RUN yarn build

FROM nginx:1.27.0 as final
COPY --from=build /app/build /usr/share/nginx/html
COPY front/nginx.conf /etc/nginx/conf.d/default.conf

ENTRYPOINT ["nginx", "-g", "daemon off;"]
