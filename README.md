Um finado bot do discord feito em C# utilizando a lib Discord.Net, com funcionalidades básicas como reprodução de música e rolagem de dados.

Para o reprodutor de músicas utilizei o SharpLink, que é um wrapper de Lavalink (node de streaming de áudio escrito em Java), responsável pelo streaming de áudio propriamente dito. O Lavalink funcionava 
como um servidor independente, onde eu o executava e conectava minha aplicação via WebSocket.

O serviço de rolagem de dados foi extremamente divertido de implementar. Grande fã de RPG de mesa, busquei adequar o serviço ao uso prático em uma partida, aceitando comandos como "6d6 + 5".

Para a hospedagem, utilizei uma instância AWS EC2 t2.micro, que fazia parte do pacote gratuito da AWS. Ficou online por cerca de 8 meses.

Favor ignorar a falta de um padrão de projeto e as gambiarras presentes no código.
