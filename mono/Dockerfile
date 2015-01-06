FROM friism/mono
MAINTAINER schaelle@gmail.com

ADD runtime runtime
EXPOSE 9000
WORKDIR runtime
ENV MONO_IOMAP all
CMD ["mono", "TerrificNet.exe"]