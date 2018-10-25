FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-obj/Docker/publish} .

RUN apt-get update
RUN apt-get install -y xvfb x11vnc x11-xkb-utils xfonts-100dpi xfonts-75dpi xfonts-scalable xfonts-cyrillic x11-apps libgdiplus libx11-dev

ENV DISPLAY :99
ENTRYPOINT ["dotnet", "FastReportCore.MVC.dll"]