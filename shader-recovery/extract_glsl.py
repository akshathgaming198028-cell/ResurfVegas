import os, re, sys
src = "/home/claude/scratchpad/ogx/assets/bin/Data"
out = "/home/claude/scratchpad/glsl"
os.makedirs(out, exist_ok=True)
count = 0
for name in sorted(os.listdir(src)):
    p = os.path.join(src, name)
    if not os.path.isfile(p):
        continue
    data = open(p, "rb").read()
    for m in re.finditer(rb"#version 100", data):
        start = m.start()
        end = data.find(b"\x00", start)
        if end == -1 or end - start > 20000:
            end = min(len(data), start + 20000)
        text = data[start:end].decode("latin-1")
        count += 1
        fn = os.path.join(out, "%03d_%s.glsl" % (count, name[:16].replace(" ", "_")))
        with open(fn, "w") as f:
            f.write(text)
print("extracted", count)
