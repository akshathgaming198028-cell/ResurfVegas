import os, re
src = "/home/claude/scratchpad/ogx/assets/bin/Data"
out = "/home/claude/scratchpad/shaderlab"
seen = {}
for name in sorted(os.listdir(src)):
    p = os.path.join(src, name)
    if not os.path.isfile(p):
        continue
    data = open(p, "rb").read().decode("latin-1")
    for m in re.finditer(r'Shader "([^"]+)" \{', data):
        sname = m.group(1)
        # ShaderLab block ends at the first line that is exactly "}" at column 0
        end = re.search(r'\n\}\s*\n', data[m.start():m.start()+20000])
        if not end:
            continue
        body = data[m.start(): m.start() + end.end()]
        if sname in seen and len(seen[sname]) >= len(body):
            continue
        seen[sname] = body
for sname, body in sorted(seen.items()):
    fn = os.path.join(out, re.sub(r'[^A-Za-z0-9]+', '_', sname).strip('_') + ".shader")
    with open(fn, "w") as f:
        f.write(body)
print("shaders:", len(seen))
