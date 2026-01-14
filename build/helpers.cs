public static partial class Program
{
    static  List<NuSpecContent> GetContent(IEnumerable<string> frameworks, ProjectCollection projects, string configuration, Func<SolutionProject, bool>? projectFilter = null) {
        projectFilter = projectFilter ?? (p => true);
        var content = new List<NuSpecContent>();
        foreach (var framework in frameworks) {
            foreach (var project in projects.SourceProjects.Where(projectFilter)) {
                Verbose("Loading package files for " + project.Name);
                var match = GetFiles(project.Path.GetDirectory() + "/bin/" + configuration + "/" + framework + "/" + project.Name +".*");
                var libFiles = match
                    .Where(f => f.GetExtension() != ".pdb")
                    .Select(f => new NuSpecContent { Source = f.FullPath, Target = $"lib/{framework}"});
                content.AddRange(libFiles);
            }
        }
        return content;
    }

    static ProjectCollection GetProjects(ConvertableFilePath slnPath, string configuration) {
        var solution = ParseSolution(slnPath);
        var projects = solution.Projects.Where(p => p.Type != "{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
        var testAssemblies = projects.Where(p => p.Name.Contains(".Tests")).Select(p => p.Path.GetDirectory() + "/bin/" + configuration + "/" + p.Name + ".dll");
        return new ProjectCollection(
            SolutionPath: slnPath,
            SourceProjects: projects.Where(p => !p.Name.Contains(".Tests")),
            TestProjects: projects.Where(p => p.Name.Contains(".Tests"))
        );
        
    }
}

public record ProjectCollection(
    ConvertableFilePath SolutionPath,
    IEnumerable<SolutionProject> SourceProjects,
    IEnumerable<SolutionProject> TestProjects)
{
    public ICollection<DirectoryPath> SourceProjectPaths => field ??= [.. SourceProjects.Select(p => p.Path.GetDirectory())];
    public ICollection<DirectoryPath> TestProjectPaths => field ??= [.. TestProjects.Select(p => p.Path.GetDirectory())];
    public ICollection<SolutionProject> AllProjects => field ??= [.. SourceProjects.Concat(TestProjects)];
    public ICollection<DirectoryPath> AllProjectPaths => field ??= [.. AllProjects.Select(p => p.Path.GetDirectory())];
}