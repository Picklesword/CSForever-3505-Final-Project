#ifndef DEP_GRAPH_H
#define DEP_GRAPH_H

#include <string>
#include <map>
#include <set>
#include <vector>


class Dependency_Graph
{

private:

	std::map<std::string, std::set<std::string> > omni_set;

	int pair_count;

	std::string dependees;

	std::string dependents;

	std::string temp_name;

	//std::set<std::string> temp_set; 

public:

	Dependency_Graph();
	~Dependency_Graph();

	int Size();

	int HasDependents(std::string);

	int HasDependees(std::string);

	std::set<std::string> GetDependents(std::string);//, std::set<std::string>*); 

	std::set<std::string> GetDependees(std::string);//, std::set<std::string>*); 

	void AddDependency(std::string, std::string);

	void RemoveDependency(std::string, std::string);

	void ReplaceDependents(std::string, std::set<std::string>);

	void ReplaceDependees(std::string, std::set<std::string>);

	int  size_dependees(std::string);


};

#endif
