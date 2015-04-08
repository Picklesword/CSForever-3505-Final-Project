/*
 * This class servers as an equivalent to string socket class on the 
 * spreadsheet client
 * Scott Young
 * Dharani Adhikari
 * Mathnew Greave
 * Jonathan Warner
 */

#ifndef SPREADSHEET_H
#define SPREADSHEET_H

using namespace std;

namespace FinalProject
{
  
  class SpreadSheet
  {

  public:

     SpreadSheet(map <string, string> data, string name);
     void save(string cell, string content);
     void delete(string cell, string content);
     void undo(void);
 
  private:
     map <string, string> data;
     int cellCounts;
  };

}//end namespace
#endif
