#include <fcntl.h>
#include <string.h>
#include <stdlib.h>
#include <errno.h>
#include <stdio.h>
#include <netinet/in.h>
#include <resolv.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <pthread.h>
#include <iostream>

using namespace std;

void* SocketHandler(void*);

int main(){

	int host_port = 2112;

  struct sockaddr_in my_addr;

  int hostsocket;
  int * p_int ;
  int err;

  socklen_t addr_size = 0;
  int* clientsocket;
  sockaddr_in sadr;
  pthread_t thread_id=0;


  hostsocket = socket(AF_INET, SOCK_STREAM, 0);
  if(hostsocket == -1)
  {
    printf("Error initializing socket %d\n", errno);
    exit(1);
  }

  my_addr.sin_family = AF_INET ;
  my_addr.sin_port = htons(host_port);
    
  memset(&(my_addr.sin_zero), 0, 8);
  my_addr.sin_addr.s_addr = INADDR_ANY ;
    
  if(bind( hostsocket, (sockaddr*)&my_addr, sizeof(my_addr)) < 0 )
  {
    cout << "ERROR binding" << endl;
   exit(1);
  }

  if(listen( hostsocket, 10) == -1 )
  {
    fprintf(stderr, "Error listening %d\n",errno);
    exit(1);
  }

  //Now lets do the server stuff

  addr_size = sizeof(sockaddr_in);
    
  while(true)
  {
    printf("waiting for a connection\n");
    if((*clientsocket = accept( hostsocket, (sockaddr*)&sadr, &addr_size))!= -1){
      printf("---------------------\nReceived connection from %s\n",inet_ntoa(sadr.sin_addr));
      pthread_create(&thread_id,0,&SocketHandler, (void*)clientsocket);
      pthread_detach(thread_id);
    }
    else{
      fprintf(stderr, "Error accepting %d\n", errno);
    }
  }
    
 return 0;
}//end main

void* SocketHandler(void* lp)
{
  int *clientsocket = (int*)lp;

  char buffer[1024];
  int buffer_len = 1024;
  int bytecount;

  memset(buffer, 0, buffer_len);
  if((bytecount = recv(*clientsocket, buffer, buffer_len, 0))== -1){
    fprintf(stderr, "Error receiving data %d\n", errno);
    exit(1);
  }
  printf("Received bytes %d\nReceived string \"%s\"\n", bytecount, buffer);
  strcat(buffer, " SERVER ECHO");

  if((bytecount = send(*clientsocket, buffer, strlen(buffer), 0))== -1){
    fprintf(stderr, "Error sending data %d\n", errno);
    exit(1);
  }
    
  printf("Sent bytes %d\n", bytecount);

}
