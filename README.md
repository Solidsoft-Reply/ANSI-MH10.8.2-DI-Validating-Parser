# Solidsoft Reply ANSI MH10.8.2 Validating Parser
The Solidsoft Reply parser for ANSI Data Identifiers is a comprehensive validating parser that conforms to the ANSI MH10.8.2 standard and parses strings containing a concatenation of DI/Value pairs using ASCII 29 as the delimiter.

ANSI defines DIs in its MH10.8.2 standard.  See [ANSI MH10.8.2 Data Identifiers - Download | MHI](https://my.mhi.org/s/store?_ga=2.190497003.1705731287.1703677310-1964478394.1703677309#/store/browse/detail/a153h000005lJuRAAU).
The parser validates each DI against the format defined for that DI.  It calls back into an Action for each DI reported to it.  For each DI, parsed data is reported as a Resolved Entity object.  Each Resolved Entity includes a collection of all errors reported while parsing the DI.
The library depends on the Solidsoft.Reply.Parsers.Common library.

This solution contains two projects - the parser itself, and a set of tests. Further information is provided in project-level read-me files and the Wiki.
