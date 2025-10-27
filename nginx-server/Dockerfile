# FROM nginx:alpine

# # Copiar configuración personalizada
# COPY nginx.conf /etc/nginx/nginx.conf

# # Crear directorio para certificados SSL
# RUN mkdir -p /etc/letsencrypt/live/app2.fazttech.store

# # Instalar certbot para Let's Encrypt (opcional - podemos hacerlo via volumes)
# RUN apk add --no-cache certbot

# # Exponer puertos
# EXPOSE 80 443

# # Health check
# HEALTHCHECK --interval=30s --timeout=3s --start-period=5s \
#   CMD wget --no-verbose --tries=1 --spider http://localhost/health || exit 1

# CMD ["nginx", "-g", "daemon off;"]

# FUNCIONAL SIN SSL

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
