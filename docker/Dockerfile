FROM debian:11

WORKDIR /app/net6.0

COPY ubuntu-latest.zip .

RUN apt update \
  && apt install -y wget unzip \
  && wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
  && dpkg -i packages-microsoft-prod.deb \
  && apt update \
  && apt install -y dotnet-runtime-6.0 \
  && unzip ubuntu-latest.zip -d /app \
  && chmod +x /app/net6.0/JboxWebdav.ConsoleApp \
  && rm packages-microsoft-prod.deb \
  && rm ubuntu-latest.zip \
  && apt remove -y wget unzip

CMD ["./JboxWebdav.ConsoleApp"]
