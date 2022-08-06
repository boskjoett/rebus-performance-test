cd Publisher
dotnet publish -c Release
docker build -t rebus-perftest-publisher .
cd ..

cd Subscriber
dotnet publish -c Release
docker build -t rebus-perftest-subscriber .
cd ..
