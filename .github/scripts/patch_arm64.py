# CI-only patch for the 64-bit (ARM64, IL2CPP) test build. Runs on the runner's checkout, never committed.
import os, re, sys

root = os.environ.get("GITHUB_WORKSPACE", ".")

# 1) Project settings: ARM64 only, IL2CPP.
ps = os.path.join(root, "ProjectSettings", "ProjectSettings.asset")
s = open(ps, encoding="utf-8").read()
if "AndroidTargetArchitectures:" not in s:
    s = s.replace("  AndroidTargetDevice: 0\n",
                  "  AndroidTargetDevice: 0\n  AndroidTargetArchitectures: 2\n", 1)
if "scriptingBackend:" not in s:
    s = s.replace("  scriptingDefineSymbols: {}\n",
                  "  scriptingBackend:\n    Android: 1\n  scriptingDefineSymbols: {}\n", 1)
open(ps, "w", encoding="utf-8").write(s)
print("settings: ARM64 + IL2CPP set")

# 2) iOS-only native bridges: turn each extern into a managed no-op so IL2CPP can link.
pat = re.compile(
    r'\[DllImport\("__Internal"\)\]\s*((?:\[Conditional\("UNITY_IPHONE"\)\]\s*)?)private static extern\s+([A-Za-z_][\w.<>\[\]]*)\s+(\w+)\s*\(([^)]*)\)\s*;')
count = 0
for dp, _, files in os.walk(os.path.join(root, "Assets")):
    for fn in files:
        if not fn.endswith(".cs"):
            continue
        p = os.path.join(dp, fn)
        src = open(p, encoding="utf-8", errors="replace").read()
        if '"__Internal"' not in src:
            continue
        def rep(m):
            attr, ret, name, args = m.group(1), m.group(2), m.group(3), m.group(4)
            if ret == "void":
                return "%sprivate static void %s(%s) { }" % (attr, name, args)
            return "%sprivate static %s %s(%s) { return default(%s); }" % (attr, ret, name, args, ret)
        new, n = pat.subn(rep, src)
        if n:
            open(p, "w", encoding="utf-8").write(new)
            count += n
print("bridges stubbed:", count)
