# Prerequisites
- .NET SDK 9.0.102

## Not using because I couldn't find system.net.quic - libmsquic:
- dotnet add package ENet-CSharp

- requires microsoft package mgr:
    - pkg manager key:  sudo wget https://packages.microsoft.com/keys/microsoft.asc -O /usr/share/keyrings/microsoft-prod.gpg
    - curl -sSL -O https://packages.microsoft.com/config/<distribution>/<version>/packages-microsoft-prod.deb
    - sudo dpkg -i packages-microsoft-prod.deb
    - rm packages-microsoft-prod.deb
    - sudo apt-get update
- sudo apt install libmsquic
- dotnet add package System.Net.Quic 

# Resources
Multi client web-socket server - https://mcguirev10.com/2019/01/15/simple-multi-client-websocket-server.html
