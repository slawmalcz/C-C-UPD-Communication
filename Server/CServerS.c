#include <arpa/inet.h>
#include <netinet/in.h>
#include <stdbool.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>

int CServer(int listen_sock){
	struct sockaddr_in client_address;
	int client_address_len = 0;
	
	int sock;
		if((sock = accept(listen_sock, (struct sockaddr *)&client_address, &client_address_len)) < 0){
			printf("Could not open a socket to accept data\n");
			return 1;
		}
		
		int n = 0;
		int len = 0, maxlen = 1024;
		char buffer[maxlen];
		char *pbuffer = buffer;
		
		printf("Client connected with ip address: %s\n",inet_ntoa(client_address.sin_addr));
		
		while((n = recv(sock, pbuffer, maxlen, 0)) > 0){
			pbuffer += n;
			maxlen -= n;
			len += n;
			
			printf("Recived: '%s'\n", buffer);
			
			send(sock, buffer, len, 0);
		}
		
		close(sock);
	return 0;
}

int CClient(char ipAdres[1024]){
	const int server_port = 25566;
	struct sockaddr_in server_address;
	int s;

	server_address.sin_family = AF_INET;
	server_address.sin_port = htons(server_port);
	inet_aton("150.254.79.213", &server_address.sin_addr.s_addr);
	
	int sock;
	if((sock = socket(PF_INET, SOCK_STREAM, 0 )) < 0){
		printf("Could not create socket\n");
		return 1;
	}
	
	if(connect(sock, (struct sockaddr*)&server_address, sizeof(server_address)) < 0){
		printf("Could not connect to server\n");
		return 1;
	}
	
	const char* data_to_send = "Test message";
	send(sock, data_to_send, strlen(data_to_send), 0);
	
	int n = 0;
	int len = 0, maxlen = 100;
	char buffer[maxlen];
	char* pbuffer = buffer;
	
	while((n = recv(sock, pbuffer, maxlen, 0)) > 0){
		pbuffer += n;
		maxlen -=n;
		len +=n;
		
		buffer[len] = '\0';
		printf("recived: '%s'\n", buffer);
	}
	
	close(sock);
	return 0;
}

int main(void)
{
	int SERVER_PORT = 25565;
	
	struct sockaddr_in server_addres;
	memset(&server_addres, 0, sizeof(server_addres));
	server_addres.sin_family = AF_INET;
	
	server_addres.sin_port = htons(SERVER_PORT);
	
	server_addres.sin_addr.s_addr = htonl(INADDR_ANY);
	
	int listen_sock;
	if((listen_sock = socket(PF_INET, SOCK_STREAM, 0)) < 0){
		printf("Could not create listen socket\n");
		return 1;
	}
	
	if((bind(listen_sock, (struct sockaddr *)&server_addres, sizeof(server_addres))) < 0){
		printf("Could not bind socket\n");
		return 1;
	}
	
	int wait_size = 16;
	
	if(listen(listen_sock, wait_size) < 0){
		printf("Could not open socket for listening\n");
		return 1;
	}
	
	struct sockaddr_in client_address;
	int client_address_len = 0;
	
	while(true){
		int sock;
		if((sock = accept(listen_sock, (struct sockaddr *)&client_address, &client_address_len)) < 0){
			printf("Could not open a socket to accept data\n");
			return 1;
		}
		
		int n = 0;
		int len = 0, maxlen = 1024;
		char buffer[maxlen];
		char *pbuffer = buffer;
		
		printf("Client connected with ip address: %s\n",inet_ntoa(client_address.sin_addr));
		
		while((n = recv(sock, pbuffer, maxlen, 0)) > 0){
			pbuffer += n;
			maxlen -= n;
			len += n;
			
			printf("Recived: '%s'\n", buffer);
			
			send(sock, buffer, len, 0);
		}
		
		close(sock);
	}
	
	close(listen_sock);
	return 0;
}
