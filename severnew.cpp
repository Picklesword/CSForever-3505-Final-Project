#include <string.h>
#include <unistd.h>
#include <stdio.h>
#include <netdb.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <iostream>
#include <fstream>
#include <strings.h>
#include <stdlib.h>
#include <string>
#include <pthread.h>

using namespace std;

void *connect(void *);

void open_spreadsheet(char *file_name);

static int connFd;

int main()
{
	int pId, portNo, listenFd;
	int size = 10;
	socklen_t len; //store size of the address
	bool loop = false;
	struct sockaddr_in svrAdd, clntAdd;

	pthread_t threadA[size];

	portNo = 2112;


	//create socket
	listenFd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (listenFd < 0)
	{
		cerr << "Cannot open socket" << endl;
		return 0;
	}

	bzero((char*)&svrAdd, sizeof(svrAdd));

	svrAdd.sin_family = AF_INET;
	svrAdd.sin_addr.s_addr = INADDR_ANY;
	svrAdd.sin_port = htons(portNo);

	//bind socket
	if (bind(listenFd, (struct sockaddr *)&svrAdd, sizeof(svrAdd)) < 0)
	{
		cerr << "Cannot bind" << endl;
		return 0;
	}

	listen(listenFd, 5);

	int noThread = 0;

	while (true)
	{
		cout << "Listening" << endl;
		socklen_t len = sizeof(clntAdd);

		//this is where client connects. svr will hang in this mode until client conn
		connFd = accept(listenFd, (struct sockaddr *)&clntAdd, &len);

		if (connFd < 0)
		{
			cerr << "Cannot accept connection" << endl;
			return 0;
		}
		else
		{
			cout << "Connection successful" << endl;
		}

		pthread_create(&threadA[noThread], NULL, connect, NULL);

		noThread++;
	}

	for (int i = 0; i < noThread; i++)
	{
		pthread_join(threadA[i], NULL);
	}
}

// The first connection received by a client will be checked here.
void *connect(void *dummyPt)
{

	cout << "Thread No: " << pthread_self() << endl;
	char buffer[300];
	char * parsed;
	bzero(buffer, 301);
	bool loop = false;
	while (!loop)
	{
		//Reads from connected client
		bzero(buffer, 301);
		read(connFd, buffer, 300);

		//Parse command for checking
		parsed = strtok(buffer, " \n\r");


		cout << parsed << endl;
		cout << strcmp(parsed, "connect") << endl;

		//Client requests to connect to server with their username
		if (!strcmp(parsed, "connect"))
		{
			send(connFd, "one more thing to understand is how big can this be\n", 45, MSG_NOSIGNAL);
		}

		//Client requests that a new user be granted access to the server
		if (!strcmp(parsed, "register user"))
		{
		}

		//Client requests to change a cell within the spreadsheet
		if (!strcmp(parsed, "cell"))
		{
		}


		if (!strcmp(parsed, "undo"))
		{
		}

		while (parsed != NULL)
		{
			cout << parsed << endl;
			parsed = strtok(NULL, " ");
		}

		string tester(buffer);



		if (tester == "exit")
			break;
	}
	cout << "\nClosing thread and conn" << endl;
	close(connFd);
}