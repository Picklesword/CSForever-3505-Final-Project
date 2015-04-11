#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include <iostream>

#include <string.h>
#include <map>

/* Holds a spreadsheet information
 */
using namespace std;

class spreadsheet
{
public:
  spreadsheet();
  spreadsheet(string filename);
  spreadsheet(const spreadsheet &other);

  ~spreadsheet();

  map<string, string> spreadsheet_data;

  string name;  

//  queue<string> command_queue;

private:
  void load_spreadsheet(std::string filename);
  void save_spreadsheet();  
  void update_spreadsheet();

};
#endif