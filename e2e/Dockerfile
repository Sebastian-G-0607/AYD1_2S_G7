FROM mcr.microsoft.com/playwright:v1.49.0-noble

WORKDIR /app

# Instalar dependencias de Node
COPY package.json package-lock.json* ./
RUN npm install

# Copiar el resto de la suite de pruebas
COPY . .

# Exponer el puerto para modo interactivo UI
EXPOSE 9323

# Por defecto ejecuta las pruebas en modo headless
CMD ["npx", "playwright", "test"]
