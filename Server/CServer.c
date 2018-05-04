#include <arpa/inet.h>
#include <netinet/in.h>
#include <stdio.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>

#define BUFLEN 1024
#define NPACK 10
#define PORT 25565

void diep(char *s)
{
	perror(s);
	exit(1);
}

int sendPackage(char *buffer,char *ipAddres){
	struct sockaddr_in si_otherSend;
	int sSend, slenSend=sizeof(si_otherSend);

	if ((sSend=socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP))==-1)
		return 1;

	memset((char *) &si_otherSend, 0, sizeof(si_otherSend));
	si_otherSend.sin_family = AF_INET;
	si_otherSend.sin_port = htons(PORT);
	if (inet_aton(ipAddres, &si_otherSend.sin_addr)==0) {
		fprintf(stderr, "inet_aton() failed\n");
		return 1 ;
	}

	if (sendto(sSend, buffer, BUFLEN, 0, &si_otherSend, slenSend)==-1)
		return 1;
	
	close(sSend);
	return 0;
}

int main(void)
{
	
	struct sockaddr_in si_me, si_other;
	int s, i, slen=sizeof(si_other);
	char buf[BUFLEN];
	char ipPoster[BUFLEN];
	char ipGeter[BUFLEN];
	int stateMachine = 0;

	if ((s=socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP))==-1)
		diep("socket");

	memset((char *) &si_me, 0, sizeof(si_me));
	si_me.sin_family = AF_INET;
	si_me.sin_port = htons(PORT);
	si_me.sin_addr.s_addr = htonl(INADDR_ANY);
	if (bind(s, &si_me, sizeof(si_me))==-1)
		diep("bind");
	while(1){
		switch(stateMachine){
			/* Waiting to recive protocol initiation package */
			case(0):
				
				if (recvfrom(s, buf, BUFLEN, 0, &si_other, &slen)==-1)
					diep("recvfrom()");
				printf("Received packet from %s:%d\nData: %s\n", 
				inet_ntoa(si_other.sin_addr), ntohs(si_other.sin_port), buf);
				sprintf(ipPoster, inet_ntoa(si_other.sin_addr));
				sprintf(ipGeter, buf);
				stateMachine = 1;
			break;
			/* Sending confirmation message about reciving initiation package */
			case(1):
				printf("Sending confirmation packet.\n");
				sprintf(buf, "Accepted connection");
				if (sendPackage(buf,ipPoster)==1)
					diep("sendto()");
				stateMachine = 2;
			break;
			/*  Checking for valid connection 
				If connection valid state 3
				if not 				state 0
			*/
			case(2):
				printf("Waiting for connection validation.\n");
				sprintf(buf, "Validate connection");
				if (sendPackage(buf,ipGeter)==1)
					diep("sendto()");
				if (recvfrom(s, buf, BUFLEN, 0, &si_other, &slen)==-1)
					diep("recvfrom()");
				if(strcmp(buf,"Validate")){
					printf("Connection valid.\n");
					sprintf(buf, "Valid connection");
					if (sendPackage(buf,ipPoster)==1)
						diep("sendto()");
					stateMachine = 3;
				}else{
					printf("Connection not valid.\n");
					sprintf(buf, "Not valid connection");
					if (sendPackage(buf,ipPoster)==1)
						diep("sendto()");
					stateMachine = 0;
				}
			break;
			/*  Start data transfer 
				Data transfer will end with empty empty frame.
			*/
			case(3):
				while(1){
					//Pobranie pakietu do buf
					if (recvfrom(s, buf, BUFLEN, 0, &si_other, &slen)==-1)
						diep("recvfrom()");
					//Sprawdzenie czy to pakiet kończacy
					if(strcmp(buf))
						break;
					//Wysłanie pakietu do odbiorcy
					if (sendPackage(buf,ipGeter)==1)
						diep("sendto()");
					//Pobranie pakietu potwierdzającego odbior
					if (recvfrom(s, buf, BUFLEN, 0, &si_other, &slen)==-1)
						diep("recvfrom()");
					if(strcmp(buf,"Accepted")){
						//Wysyłanie nadawcy pakietu potwiedzającego odbior i proźba o następny pakiet.
						sprintf(buf, "Ready");
						if (sendPackage(buf,ipGeter)==1)
							diep("sendto()");
					}else{
						diep("noAccept()");
					}
				}
				printf("Cycel compleat weiting for next transfer.\n");
				stateMachine = 0;
			break;
		}
		
	}
	close(s);
	return 0;
}
