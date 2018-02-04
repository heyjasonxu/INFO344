Azure URL: 

1. Build Trie data structure
	- create node and tree class
	- Use a dictionary(map) that points all letters to other nodes in the tree. That will help represent tree structure with muliple leaves per node.
	- Write node class with dictionary to reference letters and rest of tree. Node class will also store all the terms in database
	- Write Tree class with methods to build the tree from a list of terms, and has the ability to return the first 10 results from a search term.
	- Test the trie structure with small test dataset.
2. Filter data from Wiki Dump
	- loop threw data and remove all terms that have non english characters
	- upload file to azure blob storage
3. Create asmx web service
	- add html services, and blob key to web.config file
	- write webmethod for downloading wiki data file from Azure storage
	- write webmethod to build tree and insert all terms into tree
	- write webmethod to find top 10 results from search term
4. HTML and ajax
	- Create html file in project
	- make sure ajax and jquery are working on html file
	- create search field on html and run ajax query to search whenever a key is entered. Search webmethod is ran whenever key is entered.
	- create table to show results from query
5. Memory management
	- track memory for when term is added to tree.
	- Set a stop order to stop building tree once memory is almost full. 
	- Because I didn't use a webapp, I didn't get to manage the memory, so I just stopped building tree after set number of terms
