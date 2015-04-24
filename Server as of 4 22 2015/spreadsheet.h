#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include <iostream>

#include <string.h>
#include <map>
#include <stack>
#include <set>
#include "Dependency_Graph.h"

/* Holds a spreadsheet information
 */
using namespace std;

typedef struct
{
	string first;
	string last;
} myTuple;

class spreadsheet
{
public:
	spreadsheet();
	spreadsheet(string filename);
	spreadsheet(const spreadsheet &other);

	void save_spreadsheet(std::string filename);

	~spreadsheet();

	Dependency_Graph depgraph;

	map<string, string> cells;
	stack< pair<string, string> > stackOfChanges; //added by Dharani
	pair<string, string> undo(void); //Added by Dharani
	set< char* > Dependency_Check(set<char*>, char*, char*, int*);

	string name;

	//  queue<string> command_queue;

private:
	void load_spreadsheet(std::string filename);

	void update_spreadsheet(void);
};
#endif
