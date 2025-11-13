FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-bookworm-slim
WORKDIR /app
ARG BIN_NAME=TelegramMonitor
ARG TARGETARCH
COPY out/linux-${TARGETARCH}/ /app/
RUN chmod +x /app/${BIN_NAME}
EXPOSE 5005
ENTRYPOINT ["/app/TelegramMonitor"]