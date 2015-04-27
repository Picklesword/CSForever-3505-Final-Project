/* Final Project CS 3505 Spring 2015 April 10th-26th 
 * Dharani Adhikari, Matthew Greaves, Scott Young
 * Description: Server that listens for incoming spreadsheet clients.
 */



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
#include <ctype.h>
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

int main(int argc, char* argv[])
{

	int pId, portNo, listenFd;
	int size = 10;
	int connFd;
	socklen_t len; //store size of the address
	bool loop = false;
	struct sockaddr_in svrAdd, clntAdd;

	open_user_list();

	pthread_t threadA[size];

	portNo = 2000;

	if(argc == 2)
	{
		portNo = atoi( argv[1] );
	}


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

		pthread_create(&threadA[noThread], NULL, connect, (void *)connFd);

		noThread++;
	}

	for (int i = 0; i < noThread; i++)
	{
		pthread_join(threadA[i], NULL);
	}
}
void looper()
{
	while (true)
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
	string empt = "";
        string before;
	string rest; 
	while (!loop)
	{
		//Reads from connected client
		bzero(buffer, 301);

		//Checks to see if a socket is closed
		int toclose = read(connFd, buffer, 300);

		string readin(buffer);

		string tester(buffer);
                
		if (tester == "exit")
			break;

		int t = 0;

		// Socket has closed; close connection
		if (toclose < 0)
		{
			cout << "we recieved no bytes" << endl; 
			// Remove user from editing spreadsheet list
			string spreadsheet = users_editing[connFd];
			spreadsheet_editors[spreadsheet].remove(connFd);

			//remove user key since they are no longer editing form editing map
			users_editing.erase(connFd);

			//close the socket
			close(connFd);
			break;
		}

		//Parse command for checking
		parsed = strtok(buffer, " \n\r");//should be changed to before
		if (parsed != NULL)
		{
			//Client requests to connect to server with their username
			if (!strcmp(parsed, "connect"))
			{
				//iterate to user name
				parsed = strtok(NULL, " \n\r");

				if(parsed == NULL)
				{
                                	send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL); 
					break;
                                }
				string temp(parsed);

				if (registered_users.count(temp) == 0 && temp != "sysadmin")
				{
					send(connFd, "\nerror 4 user not registered\n", 30, MSG_NOSIGNAL);
				}
				//Open/Create Spreadsheet file
				else
				{
					if(parsed != NULL)
					{
						//iterate to spreadsheet file
						parsed = strtok(NULL, " \n\r");

						string temp(parsed);

						//if the spreadsheet is not being edited
						if (opened_spreadsheets.count(temp) == 0)
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

							to_send << "connected " << s << "\n";
							string test(to_send.str());
							send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
							connected = true;

							typedef map<string, string> map_it;
							for (map_it::iterator it = opened_spreadsheets[temp]->cells.begin(); it != opened_spreadsheets[temp]->cells.end(); it++)
							{
								stringstream cell;
								cell << "\ncell " << it->first << " " << it->second << "\n";
								string test(cell.str());
								send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
							}
						}

						else
						{

							stringstream to_send;
							int s = opened_spreadsheets[temp]->cells.size();
							to_send << "connected " << s << "\n";
							string test(to_send.str());
							send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);

							spreadsheet_editors[temp].push_back(connFd);
							users_editing.insert(make_pair(connFd, temp));

							// send cell contents
							typedef map<string, string> map_it;
							for (map_it::iterator it = opened_spreadsheets[temp]->cells.begin(); it != opened_spreadsheets[temp]->cells.end(); it++)
							{
								stringstream cell;
								cell << "\ncell " << it->first << " " << it->second << "\n";
								string test(cell.str());
								send(connFd, test.c_str(), test.size(), MSG_NOSIGNAL);
								connected = true;
							}
						}
					}
					else
					   send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL);
					
				}
			}

			//Client requests that a new user be granted access to the server
			else if (!strcmp(parsed, "register"))
			{
				// Looks at the user who is trying to register another user
				parsed = strtok(NULL, " \n\r");
				string test(parsed);
				string error_send = "\nerror 4 already registered ";
				error_send += parsed;
				error_send += "\n";

				if (!connected)
				{
					send(connFd, error_send.c_str(), error_send.size(), MSG_NOSIGNAL);
				}
				// Registers non-existing registered user if client is registered
				if (connected)//this seems not ot have correct brackets 
					if (registered_users.count(parsed) == 0)
					{
					registered_users.insert(make_pair(test, ""));
					add_user(test);
					}
				// Otherwise send the error message	  
					else
					{
						send(connFd, error_send.c_str(), error_send.size(), MSG_NOSIGNAL);
					}
				parsed = strtok(NULL, " \n\r");

                                if(parsed != NULL){
				   send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL);
				break;

                               }
			}

			// Handle undo command
			else if (!strcmp(parsed, "undo")) // Completed by Dharani
			{


                                parsed = strtok(NULL, " \n\r");

                                if(parsed != NULL){
					send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL);
					break;				   
                               }
				string spreadsheet = users_editing[connFd];
				if (opened_spreadsheets[spreadsheet]->stackOfChanges.size() != 0)
				{
					pair<string, string> undoneCell = opened_spreadsheets[spreadsheet]->undo();
					opened_spreadsheets[spreadsheet]->save_spreadsheet(spreadsheet);
					string cell_update = "cell " + undoneCell.first + " " + undoneCell.second + "\n";

					for (list<long>::iterator it = spreadsheet_editors[spreadsheet].begin(); it != spreadsheet_editors[spreadsheet].end(); it++)
					{
						long user = *it;
						send(user, cell_update.c_str(), cell_update.size(), MSG_NOSIGNAL);
					}
				}
			}

			//Client requests to change a cell within the spreadsheet
			else if (!strcmp(parsed, "cell"))
			{
				//sets the cell name as a string
				parsed = strtok(NULL, " \n\r");


                                if(parsed == NULL){

                                  send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL);                               

                                }


				string cell_name(parsed);

				if (!(isalpha(cell_name[0]) && (cell_name.size() == 3 || cell_name.size() == 2)))
					 send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL);

				//sets the cell contents as a string
				parsed = strtok(NULL, "\n\r");								
				
				string cell_contents;
                                
                                set<string> remove_set; 

				// Check if cell is empty
				if (parsed == NULL)
				{
					cell_contents = "";
				}

				else
				{
					string temp(parsed);
					cell_contents = temp;
				}

                                parsed = strtok(NULL, " \n\r");
                                if(parsed != NULL){

                                  send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL);                               

                                }

				int dep = 0;

				string spreadsheet = users_editing[connFd];

                                remove_set = opened_spreadsheets[spreadsheet]->depgraph.GetDependents(cell_name);

                                for(set<string>:: iterator itrem = remove_set.begin(); itrem != remove_set.end(); itrem++)
					opened_spreadsheets[spreadsheet]->depgraph.RemoveDependency(cell_name,*itrem);

				if (cell_contents[0] == '=')
				{
					// Normalize the cell names to prevent collisions
					for(int i = 0; i < cell_contents.size(); i++)
						cell_contents[i] = toupper(cell_contents[i]);

					cell_name[0] = toupper(cell_name[0]);


					set<char*> temp_deps;


					temp_deps = opened_spreadsheets[spreadsheet]->Dependency_Check(temp_deps, const_cast<char*> (cell_name.c_str()), const_cast<char*> (cell_contents.c_str()), &dep);

					if (!dep)
					{
						while (parsed != NULL)
						{
							if (isalpha(parsed[0]) && (strlen(parsed) == 3 || strlen(parsed) == 2)){
                                                                for(int i = 0; i < strlen(parsed); i++)
						                    parsed[i] = toupper(parsed[i]);

								opened_spreadsheets[spreadsheet]->depgraph.AddDependency(cell_name, parsed);
							}
							parsed = strtok(NULL, " =+/*-");
						}

						// Store old value on undo stack

						//If the current cell already has a value, change contents
						if (opened_spreadsheets[spreadsheet]->cells.count(cell_name) != 0)
						{
							pair<string, string> oldValue(cell_name, opened_spreadsheets[spreadsheet]->cells.at(cell_name));
							opened_spreadsheets[spreadsheet]->stackOfChanges.push(oldValue);

							if (cell_contents == "")
							{
								opened_spreadsheets[spreadsheet]->cells.erase(cell_name);
							}
							else
							{
								opened_spreadsheets[spreadsheet]->cells[cell_name] = cell_contents;
							}
							opened_spreadsheets[spreadsheet]->save_spreadsheet(spreadsheet);
						}

						//Otherwise just add it to the spreadsheet object
						else
						{
							pair<string, string> oldValue(cell_name, "");
							opened_spreadsheets[spreadsheet]->stackOfChanges.push(oldValue);
							if (cell_contents == "")
							{
								opened_spreadsheets[spreadsheet]->cells.erase(cell_name);
							}
							else
							{
								opened_spreadsheets[spreadsheet]->cells[cell_name] = cell_contents;
							}
							opened_spreadsheets[spreadsheet]->save_spreadsheet(spreadsheet);

						}

						string cell_update = "cell " + cell_name + " " + cell_contents + "\n";

						for (list<long>::iterator it = spreadsheet_editors[spreadsheet].begin(); it != spreadsheet_editors[spreadsheet].end(); it++)
						{
							long user = *it;
							send(user, cell_update.c_str(), cell_update.size(), MSG_NOSIGNAL);

						}
					}
					else
					{
						string error_send = "\nerror 1 circular dependency detected.\n";
						send(connFd, error_send.c_str(), error_send.size(), MSG_NOSIGNAL);
					}
				}
				else
				{
					//If the current cell already has a value, change contents
					if (opened_spreadsheets[spreadsheet]->cells.count(cell_name) != 0)
					{
						pair<string, string> oldValue(cell_name, opened_spreadsheets[spreadsheet]->cells.at(cell_name));
						if (cell_contents == "")
						{
							opened_spreadsheets[spreadsheet]->cells.erase(cell_name);
						}
						else
						{
							opened_spreadsheets[spreadsheet]->cells[cell_name] = cell_contents;
						}


						opened_spreadsheets[spreadsheet]->stackOfChanges.push(oldValue);

						opened_spreadsheets[spreadsheet]->save_spreadsheet(spreadsheet);


					}

					//Otherwise just add it to the spreadsheet object
					else
					{
						if (cell_contents != "")
						{
							pair<string, string> oldValue(cell_name, "");
							opened_spreadsheets[spreadsheet]->stackOfChanges.push(oldValue);
							if (cell_contents == "")
							{
								opened_spreadsheets[spreadsheet]->cells.erase(cell_name);
							}
							else
							{
								opened_spreadsheets[spreadsheet]->cells[cell_name] = cell_contents;
							}
							opened_spreadsheets[spreadsheet]->save_spreadsheet(spreadsheet);

						}
					}

					string cell_update = "cell " + cell_name + " " + cell_contents + "\n";

					for (list<long>::iterator it = spreadsheet_editors[spreadsheet].begin(); it != spreadsheet_editors[spreadsheet].end(); it++)
					{
						long user = *it;
						send(user, cell_update.c_str(), cell_update.size(), MSG_NOSIGNAL);
					}
				}
			}
		else
		{
                	send(connFd, "\nerror 2 invalid_command\n", 30, MSG_NOSIGNAL); 
                } 
		}
	}
	close(connFd);
}

void open_user_list()
{
	ifstream in("users.txt");

	if (!in.good())
	{
		ofstream out("users.txt");
		out << "sysadmin" << endl;
	}

	while (true)
	{
		string user;

		in >> user;

		if (in.fail())
			break;


		registered_users.insert(make_pair(user, ""));
	}

}

void add_user(string user)
{
	// Open and appends (app) to the end of the users.txt file
	ofstream outfile;
	outfile.open("users.txt", ofstream::out | ofstream::app);
	outfile << user << "\n";
}

