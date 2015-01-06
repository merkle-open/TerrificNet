docker stop terrificnet
docker rm terrificnet
docker run --name terrificnet -d -p 9000:9000 -v /vagrant/sample:/sample -t schaelle/terrificnet
