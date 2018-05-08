#include <arpa/inet.h>
#include <stdio.h>
#include <string.h>
#include <sys/socket.h>
#include <unistd.h>

int main(void)
{
	const char* server_name = "lst";
	const int server_port = 25565;
	
	struct sockaddr_in server_address;
	memset(&server_address, 0, sizeof(server_address));
	server_address.sin_family = AF_INET;
	
	inet_pton(AF_INET, server_name, &server_address.sin_addr);
	
	server_address.sin_port = htons(server_port);
	
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
