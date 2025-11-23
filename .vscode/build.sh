projects=(
            "CBCI-Api"
            "CustomItemExamples"
            # "Project2"
            # "Project3"
)

max_jobs=$(nproc) # or set manually, e.g., 4
current_jobs=1

build_project() {
    project=$1
    echo "Building project: $project"
    cd "$project" || { echo "Failed to enter $project"; exit 1; }

    if ! (dotnet build -v quiet > /dev/null 2>&1); then
        echo -e "\033[31mBuild failed for $project\033[0m"
        exit 1
    else
        echo -e "\033[32mBuild succeeded for $project\033[0m"
    fi

    cp -f obj/Debug/net481/"$project".dll ../../TestPlugin/server/SL/LabAPI/plugins/global/
    cp -f obj/Debug/net481/"$project".dll ../releases/
    rm -rf ./release/

    cd ..
}

for project in "${projects[@]}"; do
    build_project "$project" &
    ((current_jobs++))

    # Wait if we've reached max parallel jobs
    if (( current_jobs >= max_jobs )); then
        wait -n  # wait for any job to finish
        ((current_jobs--))
    fi
done

# Wait for all background jobs to finish
wait

echo -e "\033[0mDone. All files stored in ./releases/"
