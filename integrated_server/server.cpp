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

void open_user_list();

void add_user(string user);

pthread_mutex_t lock;

// Key: Registered Users. Value: None
map<string, string> registered_users;

// Key: Spreadsheet Name. Value: list of clients editing instance of spreadsheet 
map<string, list<long> > spreadsheet_editors;

// Key: Spreadsheet Name. Value: Pointer to spreadsheet object 
map<string, spreadsheet*> opened_spreadsheets;

// Key: User. Value: Spreadsheet Name
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
    bool connected = false;
    string empt =""; 
    while(!loop)
    {    
        //Reads from connected client
        bzero(buffer, 301);                
        read(connFd, buffer, 300);
        
        //Parse command for checking
        parsed = strtok (buffer, " \n\r");
        if(parsed != NULL)
	{

	//Client requests to connect to server with their username
        if(!strcmp(parsed, "connect"))
        {
	  //iterate to user name
	  parsed = strtok (NULL, " \n\r");	 	  
	  string temp(parsed);
          
	  //if connecting client user is not registered
	  if(registered_users.count(temp) == 0 && temp != "sysadmin")
	  {
	    send(connFd, "error 4 blah\n", 10, MSG_NOSIGNAL);
          }
          //Open/Create Spreadsheet file
	  else
	  {

  	    //iterate to spreadsheet file
	    parsed = strtok (NULL, " \n\r");
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
	      
	      //return conncted command of size of spreadhseet
	      int s = opened_spreadsheets[temp]->cells.size();
 	      stringstream to_send;
   
              to_send <<"connected " << s << "\n";
	      cout << "connected " << endl;
              string test(to_send.str());
	      send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
	      connected = true;

	      typedef map<string, string> map_it;
	      for(map_it::iterator it = opened_spreadsheets[temp]->cells.begin(); it != opened_spreadsheets[temp]->cells.end(); it++)
	      {
	        stringstream cell;
		cell << "\ncell "<<it->first << " " << it->second << "\n";
	        string test(cell.str());
	        send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
	      }
	    }

            else
	    {

	      stringstream to_send;
	      int s = opened_spreadsheets[temp]->cells.size();
	      to_send <<"connected "<<s<<"\n";
	      string test(to_send.str());
	      send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
	      
  	      // send cell contents
	      typedef map<string, string> map_it;
	      for(map_it::iterator it = opened_spreadsheets[temp]->cells.begin(); it != opened_spreadsheets[temp]->cells.end(); it++)
	      {
	        stringstream cell;
		cell << it->first << " " << it->second << "\n";
	        string test(cell.str());
	        send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
	        connected = true;                
	      }	      
	    }	    
	  }
        }
	
	//Client requests that a new user be granted access to the server
	if(!strcmp(parsed, "register"))
	{
	  // Looks at the user who is trying to register another user
	  parsed = strtok (NULL, " \n\r");
	  string test(parsed);
	  string error_send = "error 4 ";
	  error_send  += parsed;
	  error_send += "\n";
          cout << "connected " <<connected << endl;
	  if(!connected)
          {
            send(connFd, error_send.c_str(), 20, MSG_NOSIGNAL);
	  }
	  // Registers non-existing registered user if client is registered
	  if(registered_users.count(parsed) == 0 || connected)
	  {
            add_user(test);	    
	  }
	  // Otherwise send the error message	  
	  else
 	  {
 	    send(connFd, error_send.c_str(), 20,  MSG_NOSIGNAL);
	  }
	}

        if(!strcmp(parsed, "undo")) // Completed by Dharani
	{
      	  string spreadsheet = users_editing[connFd];
	  pair<string, string> undoneCell = opened_spreadsheets[spreadsheet]->undo();
	  string cell_update = "cell " + undoneCell.first + " " +  undoneCell.second + "\n";
            
          for(list<long>::iterator it = spreadsheet_editors[spreadsheet].begin(); it != spreadsheet_editors[spreadsheet].end(); it++)
          {
	      long user = *it;
   	      send(user , cell_update.c_str(), cell_update.size(), MSG_NOSIGNAL);
	      }  
	}

	//Client requests to change a cell within the spreadsheet
	if(!strcmp(parsed,"cell"))
	{
          //sets the cell name as a string
	  parsed = strtok (NULL, " \n\r");
	  string cell_name(parsed);
          cout << cell_name << endl;
                    
	  //sets the cell contents as a string
	  parsed = strtok (NULL, "\n\r");
          string cell_contents(parsed);
          cout << cell_contents << endl;  

            int dep = 0; 

           string spreadsheet = users_editing[connFd];
          
          if(cell_contents[0] == '='){

             set<char*> temp_deps;                

            cout<< "got right before deps check" << endl; 

            temp_deps = opened_spreadsheets[spreadsheet]->Dependency_Check(temp_deps, const_cast<char*> (cell_name.c_str()),const_cast<char*> (cell_contents.c_str()), &dep);            

            cout << "past dep check" << endl; 
			if(!dep){

                  cout << "made it right before iffy line" << endl; 
		  parsed = strtok(NULL, " =+/*-");//this line may require saving the previous contents as a separate char*
                   cout << "made it past iffy line" << endl;  
          while(parsed !=NULL){

                 
		   opened_spreadsheets[spreadsheet]->depgraph.AddDependency(cell_name,parsed); 
		   cout << "made it past adding deps" << endl; 
          
          parsed = strtok(NULL, " =+/*-");     
               
          
          
          }   
          } else{

		  //error handle 
		  
          }		  
          } else{
 
           opened_spreadsheets[spreadsheet]->depgraph.AddDependency(cell_name,cell_contents);

            
	   
            //If the current cell already has a value, change contents
            if(opened_spreadsheets[spreadsheet]->cells.count(cell_name) != 0)
	      opened_spreadsheets[spreadsheet]->cells[cell_name] = cell_contents;

	    //Otherwise just add it to the spreadsheet object
            else
   	      opened_spreadsheets[spreadsheet]->cells.insert(make_pair(cell_name, cell_contents));

            string cell_update = "cell " + cell_name + " " + cell_contents + "\n";
            
            for(list<long>::iterator it = spreadsheet_editors[spreadsheet].begin(); it != spreadsheet_editors[spreadsheet].end(); it++)
            {
	      long user = *it;
   	      send(user , cell_update.c_str(), cell_update.size(), MSG_NOSIGNAL);
            }   		  
          }
      

          
	  // Check if the cell change requested would create a circular dependency
	 // bool delete_me = false;
          //if(delete_me)
	  //{
	    // send error message
         // } 
	  /*else
	  {
	    //Returns the value of the spreadsheet the client is editing
	    string spreadsheet = users_editing[connFd];
	   
            //If the current cell already has a value, change contents
            if(opened_spreadsheets[spreadsheet]->cells.count(cell_name) != 0)
	      opened_spreadsheets[spreadsheet]->cells[cell_name] = cell_contents;

	    //Otherwise just add it to the spreadsheet object
            else
   	      opened_spreadsheets[spreadsheet]->cells.insert(make_pair(cell_name, cell_contents));

            string cell_update = "cell " + cell_name + " " + cell_contents + "\n";
            
            for(list<long>::iterator it = spreadsheet_editors[spreadsheet].begin(); it != spreadsheet_editors[spreadsheet].end(); it++)
            {
	      long user = *it;
   	      send(user , cell_update.c_str(), cell_update.size(), MSG_NOSIGNAL);
            }   
	  }*/

	  cout << "hi" << endl;
	}

	
	
}
      
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

void add_user(string user)
{
  // Open and appends (app) to the end of the users.txt file
  ofstream outfile;
  outfile.open ("users.txt", ofstream::out | ofstream::app);
  outfile << user << "\n";
}
