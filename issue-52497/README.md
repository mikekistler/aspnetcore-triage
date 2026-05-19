# Triage for Issue 52497

This project explores the behavior of ASP.NET Core minimal APIs when binding enum values from different sources (route parameters, query parameters, headers, form data, and JSON body) and how case sensitivity affects enum parsing, particularly in the context of issue 52497 where enum route parameters are parsed case-sensitively even though the rest of the routing stack is case-insensitive.

## Findings

- Enum values for route, query, header, and form parameters are parsed case-sensitively.
- Enum values in JSON body payloads are parsed case-insensitively when using the `JsonStringEnumConverter`.

