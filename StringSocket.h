/*
 * This class servers as an equivalent to string socket class on the 
 * spreadsheet client
 * Scott Young
 * Dharani Adhikari
 * Mathnew Greave
 * Jonathan Warner
 */

#ifndef STRINGSOCKET_H
#define STRINGSOCKET_H

using namespace std;

namespace FinalProject
{
  
  class StringSocket
  {

  public:

     StringSocket(socket s, Encoding e);
     void beginSend(string message, CallbackType callback, int clientID);
     void beginReceive(CallbackType callback, int clientID);
     void close();
 
  private:
     int socket;
     string outGoingmsg;
     string incomingmsg;
     Encoding utf8;
  };

}//end namespace
#endif
