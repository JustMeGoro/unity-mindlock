import sys
from pathlib import Path

p = Path(r"c:\Users\demba\Documents\GitHub\unity-mindlock\Mindlock\Assets\Scenes\SampleScene.unity")
backup = p.with_suffix('.unity.bak')
if not p.exists():
    print('File not found:', p)
    sys.exit(1)

text = p.read_text(encoding='utf-8')
backup.write_text(text, encoding='utf-8')

lines = text.splitlines()
out = []
i = 0
state = 'normal'  # normal, skip_upstream, keep_stashed
while i < len(lines):
    line = lines[i]
    if line.startswith('<<<<<<<'):
        # skip upstream
        state = 'skip_upstream'
        i += 1
        continue
    if line.startswith('=======') and state == 'skip_upstream':
        state = 'keep_stashed'
        i += 1
        continue
    if line.startswith('>>>>>>>') and state == 'keep_stashed':
        state = 'normal'
        i += 1
        continue
    if state == 'normal':
        out.append(line)
    elif state == 'skip_upstream':
        # skip lines until =======
        i += 1
        continue
    elif state == 'keep_stashed':
        out.append(line)
    i += 1

cleaned = '\n'.join(out) + '\n'
p.write_text(cleaned, encoding='utf-8')
print('Backup written to', backup)
print('Cleaned file written to', p)
