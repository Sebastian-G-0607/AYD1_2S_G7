# ---------- Build stage ----------
# Use Node.js LTS Alpine for building the Vue.js application
ARG NODE_VERSION=24-alpine
FROM node:${NODE_VERSION} AS builder

WORKDIR /app

# 1. Copy package manifests first for optimal Docker layer caching
COPY package.json package-lock.json* ./

# 2. Install dependencies using npm ci (reproducible, frozen lockfile)
#    --mount=type=type=cache caches .npm directory between builds
RUN --mount=type=cache,target=/root/.npm npm install --legacy-peer-deps

# 3. Copy source files
COPY . .

# 4. Run production build (sets NODE_ENV=production automatically)
#    Output goes to /app/dist (Vite default)
RUN npm run build

# ---------- Runtime stage ----------
# Use nginx Alpine for serving static files
# This is a lightweight image (~30MB final size)
FROM nginx:alpine AS runner

# 1. Fix permissions for non-root user (UID 101 in nginx)
#    The nginx alpine image creates directories at startup,
#    but we pre-create and set ownership here for reliability
RUN mkdir -p /var/run/nginx \
    && chown -R nginx:nginx /var/cache/nginx \
    && chown -R nginx:nginx /var/run/nginx

# 2. Copy custom main nginx configuration (pid, events, http setup)
COPY nginx-main.conf /etc/nginx/nginx.conf

# 3. Copy custom server block configuration (SPA routing, security, gzip)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 4. Copy built assets from builder stage to nginx's HTML directory
COPY --from=builder /app/dist /usr/share/nginx/html

# 5. Ensure proper ownership for non-root user
RUN chown -R nginx:nginx /usr/share/nginx/html

# 6. Expose port 8080 (non-privileged port, works for non-root user)
#    Note: nginx alpine by default listens on port 80,
#    but we override this in nginx.conf to listen on 8080
EXPOSE 8080

# 7. Run as non-root user (nginx user, UID 101)
USER nginx

# 8. Health check to verify the server is responding
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD wget -qO- http://localhost:8080/ || exit 1

# 9. Start nginx (daemon off for foreground execution in Docker)
CMD ["nginx", "-g", "daemon off;"]