# The Report

## System's Perspective

### Design and architecture of your _ITU-MiniTwit_ systems

The technologies we selected to recreate the legacy flask application ITU-MiniTwit, were C# with the ASP.NET framework for the server and Postgresql as the database. The descision of using ASP.NET was made based on that the team had prior experience using it, which enabled us to quickly implement the features from the Flask application. Another reason for our selection is that with the performance enchancements of .NET 7, this is a very well performing framework (no. 14 in this benchmark: https://www.techempower.com/benchmarks/#section=data-r21). This can also be seen in our groups solution being one of the top performing in this chart: http://104.248.134.203/chart.svg.

We used Razor pages to render the web pages on the server side. We also considered creating a single page application as a frontend, such as with Blazor or React, as this would be the more modern approach. However we stuck to using razor pages as we wanted to replicate the original flask application as much as possible.

### All dependencies of your _ITU-MiniTwit_ systems on all levels of abstraction and development stages

That is, list and briefly describe all technologies and tools you applied and depend on.

### Important interactions of subsystems

### Describe the current state of your systems, for example using results of static analysis and quality assessments

### Finally, describe briefly, if the license that you have chosen for your project is actually compatible with the licenses of all your direct dependencies

Double check that for all the weekly tasks (those listed in the schedule) you include the corresponding information.

### Process' perspective

A description and illustration of:

- How do you interact as developers?
- How is the team organized?
- A complete description of stages and tools included in the CI/CD chains.
  - That is, including deployment and release of your systems.
- Organization of your repositor(ies).
  - That is, either the structure of of mono-repository or organization of artifacts across repositories.
  - In essence, it has to be be clear what is stored where and why.
- Applied branching strategy.
- Applied development process and tools supporting it
  - For example, how did you use issues, Kanban boards, etc. to organize open tasks
- How do you monitor your systems and what precisely do you monitor?
- What do you log in your systems and how do you aggregate logs?
- Brief results of the security assessment.
- Applied strategy for scaling and load balancing.
- In case you have used AI-assistants for writing code during your project or to write the report:
  - Explain which system(s) you used during the project.
  - Reflect how it supported/hindered your process.

In essence it has to be clear how code or other artifacts come from idea into the running system and everything that happens on the way.

### Lessons Learned Perspective

Describe the biggest issues, how you solved them, and which are major lessons learned with regards to:

- evolution and refactoring
- operation, and
- maintenance

of your _ITU-MiniTwit_ systems. Link back to respective commit messages, issues, tickets, etc. to illustrate these.

Also reflect and describe what was the "DevOps" style of your work. For example, what did you do differently to previous development projects and how did it work?
