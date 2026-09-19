# OPC UA Documentation & API References

Consult this file before writing OPC UA code. Prefer official sources over guessing.

## Primary sources
- **OPC Foundation UA-.NETStandard (official SDK used here):**
  https://github.com/OPCFoundation/UA-.NETStandard
  - Use for API lookup: `webfetch` concrete source files from this repo (e.g. `Stack/Opc.Ua.Core/Types/...`) when a symbol is unclear.
  - Used version in this repo: `OPCFoundation.NetStandard.Opc.Ua` v1.5.378.152 (see `Directory.Packages.props`).
- **OPC UA Specifications portal:**
  https://opcfoundation.org/developer-tools/documents/?type=Specification
  - Source of truth for the data model, address space, methods, and naming rules.

## Recommended lookup workflow
1. Know what you need (Node type, Attributes, Service call, DataType).
2. Prefer the official spec link above for semantic/behavioral questions.
3. For exact C# API signatures, `webfetch` the matching source file under `OPCFoundation/UA-.NETStandard` on GitHub instead of guessing.
4. Only fall back to search engines/community answers if both official sources fail.

## Key concepts in this codebase
- An entity = C# class annotated with `OpcUaEntityAttribute`; the Roslyn generator produces client/server artifacts.
- Server implementation lives in `Hoeyer.OpcUa.Server` (`OpcEntityServer`, `StartableEntityServer`).
- Client services browse/write/read entities via generated interfaces (`IEntityBrowser<T>`, etc.).

## When to escalate
If official docs do not answer the question, say so explicitly in your summary instead of
silently inventing OPC UA semantics.