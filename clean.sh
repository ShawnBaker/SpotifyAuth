find . -name obj -type d -prune -exec rm -rf {} \;
find . -name bin -type d -prune -exec rm -rf {} \;
find . -name .vs -type d -prune -exec rm -rf {} \;

find . -name *.user -exec rm -f {} \;
