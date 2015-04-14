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
#include <map>
#include <list>
#include <sstream>
#include "spreadsheet.h"

using namespace std;

void *connect(void *);

void open_spreadsheet(char *file_name);

void looper();

void open_user_list();

pthread_mutex_t lock;

map<string, string> registered_users;

map<string, list<long> > spreadsheet_editors;

map<string, spreadsheet*> opened_spreadsheets;

map<long, string> users_editing;


ifstream users("users.txt");

int main()
{
    int pId, portNo, listenFd;
    int size = 10;
    int connFd;
    socklen_t len; //store size of the address
    bool loop = false;
    struct sockaddr_in svrAdd, clntAdd;

    open_user_list();    

    pthread_t threadA[size];
    
    portNo = 2112;
    
    
    //create socket
    listenFd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    
    if(listenFd < 0)
    {
        cerr << "Cannot open socket" << endl;
        return 0;
    }
    
    bzero((char*) &svrAdd, sizeof(svrAdd));
    
    svrAdd.sin_family = AF_INET;
    svrAdd.sin_addr.s_addr = INADDR_ANY;
    svrAdd.sin_port = htons(portNo);
    
    //bind socket
    if(bind(listenFd, (struct sockaddr *)&svrAdd, sizeof(svrAdd)) < 0)
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
        
        pthread_create(&threadA[noThread], NULL, connect, (void *) connFd); 
        
        noThread++;
    }
    
    for(int i = 0; i < noThread; i++)
    {
        pthread_join(threadA[i], NULL);
    }    
}
void looper()
{
  while(true)
  {
  }
}
// The first connection received by a client will be checked here.
void *connect(void *client_sock)
{
    long connFd = (long)client_sock;
    
    cout << "Thread No: " << pthread_self() << endl;
    char buffer[300];
    char * parsed;
    bzero(buffer, 301);
    bool loop = false;
    string empt =""; 
    while(!loop)
    {    
        //Reads from connected client
        bzero(buffer, 301);                
        read(connFd, buffer, 300);
        
        //Parse command for checking
        parsed = strtok (buffer, "|\n\r");
        if(parsed != NULL)
	{

	//Client requests to connect to server with their username
        if(!strcmp(parsed, "connect"))
        {
	  //iterate to user name
	  parsed = strtok (NULL, "|\n\r");	 	  
	  string temp(parsed);
          
	  //if connecting client user is not registered
	  if(registered_users.count(temp) == 0 && temp != "sysadmin")
	  {
	    send(connFd, "error 4\n", 10, MSG_NOSIGNAL);
          }
          //Open/Create Spreadsheet file
	  else
	  {
  	    //iterate to spreadsheet file
	    parsed = strtok (NULL, "|\n\r");
	    string temp(parsed);

	    //if the spreadsheet is not being edited
 	    if(opened_spreadsheets.count(temp) == 0)
	    {
	      spreadsheet *item = new spreadsheet(temp);
	      list<long> editors;
	      editors.push_back(connFd);
	      spreadsheet_editors.insert(make_pair(temp, editors));
	      opened_spreadsheets.insert(make_pair(temp, item));
	      users_editing.insert(make_pair(connFd, temp));

	      send(connFd, "cofirm connection|0", 20, MSG_NOSIGNAL);
	    }

            else
	    {
	      stringstream to_send;
	      int s = opened_spreadsheets[temp]->cells.size();
	      to_send <<"confirm connection|"<<s<<"\n";
	      string test(to_send.str());
	      send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
	      
  	      // send cell contents
	      typedef map<string, string> map_it;
	      for(map_it::iterator it = opened_spreadsheets[temp]->cells.begin(); it != opened_spreadsheets[temp]->cells.end(); it++)
	      {
	        stringstream cell;
		cell << it->first << "|" << it->second << "\n";
	        string test(cell.str());
	        send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
	      }
	      
	    }
  	    
	  }

        }
	
	//Client requests that a new user be granted access to the server
	if(!strcmp(parsed, "register user"))
	{
	}

	//Client requests to change a cell within the spreadsheet
	if(!strcmp(parsed,"cell"))
	{
	  pthread_mutex_lock(&lock);
 	  looper();
	  pthread_mutex_unlock(&lock);
	//function to update spreadsheet (spreadsheet name);
	}

	
	if(!strcmp(parsed, "undo"))
	{
	}
}
/*
        while( parsed != NULL)
{
        cout << parsed << endl;
	parsed = strtok (NULL, "|\n\r");
}
*/      
        string tester (buffer);
        
             
 
        if(tester == "exit")
            break;
    }
    cout << "\nClosing thread and conn" << endl;
    close(connFd);
}

void open_user_list()
{
  ifstream in("users.txt");

  if(!in.good())
  {
    ofstream out ("users.txt");
    out <<  "sysadmin" << endl;
  }

  while(true)
  {
    string user;

    in >> user;

    if(in.fail())
      break;

    
    registered_users.insert(make_pair(user, ""));
    cout << user << endl;
  }
	
}


