FROM nginx:alpine

# Eliminar configuración por defecto
RUN rm /etc/nginx/conf.d/default.conf

# Copiar tu configuración
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Exponer puerto 8080
EXPOSE 8080