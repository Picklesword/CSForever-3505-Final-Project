all:	server
server:	server.cpp spreadsheet.cpp Dependency_Graph.cpp
	g++ server.cpp spreadsheet.cpp Dependency_Graph.cpp -lpthread

run:
	./a.out
clean:
	rm -f *.o *.out *~
