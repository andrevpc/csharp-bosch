﻿using System.Collections.Generic;
using System.Linq;
using static System.Console;
using System;

int _count = 0;
Universidade uni = new Universidade();

Pesquisa("As disciplinas com mais de 10 caractéres no nome.");
Pesquisador.Pesquisa1(uni);
WriteLine();

Pesquisa("Os departamentos, em ordem alfabética, com o número de disciplinas.");
Pesquisador.Pesquisa2(uni);
WriteLine();

Pesquisa("Liste os alunos e suas idades com seus respectivos professores.");
Pesquisador.Pesquisa3(uni);
WriteLine();

Pesquisa("Liste os professores e seus salários com seus respectivos alunos.");
Pesquisador.Pesquisa4(uni);
WriteLine();

Pesquisa("Top 10 Professores com mais alunos da universidade.");
Pesquisador.Pesquisa5(uni);
WriteLine();

Pesquisa("Considerando que todo aluno custa 300 reais mais o salário dos seus professores"
    + " divido entre seus colegas de classe. Liste os alunos e seus respectivos custos.");
Pesquisador.Pesquisa6(uni);
WriteLine();

ReadKey(true);
void Pesquisa(string texto) => WriteLine($"Pesquisa {++_count}. {texto}\n");

public class Pesquisador
{
    /// <summary>
    /// As disciplinas com mais de 10 caractéres no nome
    /// </summary>

    public static void Pesquisa1(Universidade uni)
    {
        var mais = uni.Disciplinas.Where(d => d.Nome.Length > 10);
        foreach(var d in mais)
        {
            Console.WriteLine(d.Nome);
        }
        // foreach(Disciplina d in uni.Disciplinas)
        // {
        //     if(d.Nome.Length > 10)
        //     {
        //         Console.WriteLine(d.Nome);
        //     }
        // }
    }

    /// <summary>
    /// Os departamentos, em ordem alfabética, com o número de disciplinas.
    /// </summary>
    public static void Pesquisa2(Universidade uni)
    {
        var depdis = uni.Disciplinas.Join(uni.Departamentos,
                            dis => dis.DepartamentoID,
                            dep => dep.ID,
                            (dis,dep) => new
                            {
                                NomeDep = dep.Nome,
                                NomeDis = dis.Nome,
                            });
        var ord = depdis.GroupBy(dis => dis.NomeDep)
                        .Select(dep => new {
                            NomeDep = dep.Key,
                            Disciplinas = dep.Count()
                        }
                        ).OrderBy(c => c.NomeDep);

        foreach(var dep in ord)
        {
            Console.WriteLine($"Departamento: {dep.NomeDep} -- Quantidade de disciplinas: {dep.Disciplinas}");
        }

        // uni.Departamentos.OrderBy(c => c.Nome);
        // foreach(var dep in uni.Departamentos)
        // {
        //     int count = 0;
        //     foreach(var dis in uni.Disciplinas)
        //     {
        //         if (dep.ID == dis.DepartamentoID)
        //         {
        //             count+=1;
        //         }
        //     }

        //     Console.WriteLine($"Departamento: {dep.Nome} tem {count} disciplinas");
        // }
    }

    /// <summary>
    /// Liste os alunos com seus respectivos professores
    /// </summary>
    public static void Pesquisa3(Universidade uni)
    {
        var professornaturma = uni.Turmas.Join(uni.Professores,
                                            turma => turma.ProfessorID,
                                            professor => professor.ID,
                                            (turma, professor) => new
                                            {
                                                TurmaID = turma.ID,
                                                Professor = professor.Nome
                                            });
        // var professornaturma = (from tur in uni.Turmas
        //                         join pro in uni.Professores
        //                         on tur.ProfessorID equals pro.ID
        //                         select new { TurmaID = tur.ID, Professor = pro.Nome });
        
        var alunocomprofessor = uni.Alunos.Select(a =>
                                                {
                                                    var professores = professornaturma
                                                        .Where(p => a.TurmasMatriculados.Contains(p.TurmaID))
                                                        .DistinctBy(p => p.Professor)
                                                        .Select(p => p.Professor);
                                                    return new { Aluno = a.Nome, Professores = professores };
                                                });

        // var alunocomprofessor = (from aluno in uni.Alunos
        //                         let professores = professornaturma where );
        
        // var sla  = professornaturma
        //     .GroupBy(p => p.Professor)
        //     .Select(g => new { Professor = g.Key, Qtd = g.Count()});

        // foreach (var item in sla)
        // {
        //     WriteLine(item);
        // }
        foreach (var item in alunocomprofessor)
        {
            WriteLine(item.Aluno);
            foreach (var item2 in item.Professores)
            {
                Write($"{item2}, ");
            }
            WriteLine("\n");
        }
    }

    /// <summary>
    /// Liste o número de alunos que cada professor possui.
    /// </summary>
    public static void Pesquisa4(Universidade uni)
    {
        var alunoporturma = uni.Turmas.Select(t => new{
            ProfessorID = t.ProfessorID,
            QtdeAlunos = uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID)).Count()
        }).Join(uni.Professores,
                a => a.ProfessorID,
                p => p.ID,
                (a, p) => new{
                    p.Nome,
                    a.QtdeAlunos
                });


        foreach (var item in alunoporturma)
        {
            Console.WriteLine($"Professor: {item.Nome} -- Quanti: {item.QtdeAlunos}");
        }
    }

    /// <summary>
    /// Top 10 Professores com mais alunos da universidade
    /// </summary>
    public static void Pesquisa5(Universidade uni)
    {
        var alunoporturma = uni.Turmas.Select(t => new{
            ProfessorID = t.ProfessorID,
            QtdeAlunos = uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID)).Count()
        }).Join(uni.Professores,
                a => a.ProfessorID,
                p => p.ID,
                (a, p) => new{
                    p.Nome,
                    a.QtdeAlunos
                }).OrderByDescending(p => p.QtdeAlunos).Take(10);


        foreach (var item in alunoporturma)
        {
            Console.WriteLine($"Professor: {item.Nome} -- Quanti: {item.QtdeAlunos}");
        }
    }

    /// <summary>
    /// Considerando que todo aluno custa 300 reais mais o salário dos seus professores
    /// divido entre seus colegas de classe. Liste os alunos e seus respectivos custos
    /// </summary>
    public static void Pesquisa6(Universidade uni)
    {
        var custodaturma = uni.Turmas.Select(t => new{
            t.ID,
            custo = uni.Professores.First(p => p.ID == t.ProfessorID).Salario / uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID)).Count()
        });

        var alunoporsala = uni.Alunos
            .Select(a =>
            {
                var custo = custodaturma
                    .Where(t => a.TurmasMatriculados.Contains(t.ID))
                    .Sum(t => t.custo);
                return new{
                    a.Nome,
                    Custo = 300 + custo
                };
            }).OrderByDescending(a => a.Custo);

        foreach (var item in alunoporsala)
        {
            Console.WriteLine($"Aluno: {item.Nome, -30} -- Custo: {Math.Round(item.Custo, 2), 8}");
        }
    }
}

public class Elemento
{
    public int ID { get; set; }
}

public class Pessoa : Elemento
{
    public string Nome { get; set; }
    public int Idade { get; set; }
}

public class Professor : Pessoa
{
    public int DepartamentoID { get; set; }
    public decimal Salario { get; set; }
}

public class Departamento : Elemento
{
    public string Nome { get; set; }
}

public class Disciplina : Elemento
{
    public int DepartamentoID { get; set; }
    public string Nome { get; set; }
}

public class Turma : Elemento
{
    public int DisciplinaID { get; set; }
    public int ProfessorID { get; set; }
    public string Codigo { get; set; }
}

public class Aluno : Pessoa
{
    public List<int> TurmasMatriculados { get; set; } = new List<int>();
}

public class Universidade
{
    public IEnumerable<Departamento> Departamentos { get; set; } = new List<Departamento>()
    {
        new Departamento() { ID = 1, Nome = "DAMAT" },
        new Departamento() { ID = 2, Nome = "DAFIS" },
        new Departamento() { ID = 3, Nome = "DAINF" },
        new Departamento() { ID = 4, Nome = "DAELN" }
    };

    public IEnumerable<Disciplina> Disciplinas { get; set; } = new List<Disciplina>()
    {
        new Disciplina() { ID = 1, Nome = "Cálculo 1", DepartamentoID = 1 },
        new Disciplina() { ID = 2, Nome = "Cálculo 2", DepartamentoID = 1 },
        new Disciplina() { ID = 3, Nome = "Cálculo 3", DepartamentoID = 1 },
        new Disciplina() { ID = 4, Nome = "Física 1", DepartamentoID = 2 },
        new Disciplina() { ID = 5, Nome = "Física 2", DepartamentoID = 2 },
        new Disciplina() { ID = 6, Nome = "Física 3", DepartamentoID = 2 },
        new Disciplina() { ID = 7, Nome = "Técnicas de Programação 1", DepartamentoID = 3},
        new Disciplina() { ID = 8, Nome = "Técnicas de Programação 2", DepartamentoID = 3},
        new Disciplina() { ID = 9, Nome = "Eletricidade", DepartamentoID = 4 },
        new Disciplina() { ID = 10, Nome = "Oficinas de Integração", DepartamentoID = 4 },
        new Disciplina() { ID = 11, Nome = "Estrutura de Dados 2", DepartamentoID = 3 },
        new Disciplina() { ID = 12, Nome = "Física 4", DepartamentoID = 2 }
    };

    public IEnumerable<Professor> Professores { get; set; } = new List<Professor>()
    {
        new Professor() { ID = 1, DepartamentoID = 1, Nome = "Fábio Dorini", Idade = 34, Salario = 11000 },
        new Professor() { ID = 2, DepartamentoID = 1, Nome = "Inácio", Idade = 45, Salario = 7000 },
        new Professor() { ID = 3, DepartamentoID = 1, Nome = "Roni", Idade = 38, Salario = 12000 },
        new Professor() { ID = 4, DepartamentoID = 3, Nome = "Leiza Dorini", Idade = 30, Salario = 8000 },
        new Professor() { ID = 5, DepartamentoID = 2, Nome = "Rafael Barreto", Idade = 32, Salario = 15000 },
        new Professor() { ID = 6, DepartamentoID = 4, Nome = "Jean Simão", Idade = 38, Salario = 9600 },
        new Professor() { ID = 7, DepartamentoID = 4, Nome = "Razera", Idade = 29, Salario = 13000 },
        new Professor() { ID = 8, DepartamentoID = 4, Nome = "Cezar Sanchez", Idade = 31, Salario = 8500 },
        new Professor() { ID = 9, DepartamentoID = 3, Nome = "Bogdan Nassu", Idade = 33, Salario = 14000 },
        new Professor() { ID = 10, DepartamentoID = 3, Nome = "Bogado", Idade = 48, Salario = 5000 }
    };

    public IEnumerable<Turma> Turmas { get; set; } = new List<Turma>()
    {
        new Turma() { ID = 1, Codigo = "S71", DisciplinaID = 1, ProfessorID = 1 },
        new Turma() { ID = 2, Codigo = "S77", DisciplinaID = 1, ProfessorID = 1 },
        new Turma() { ID = 3, Codigo = "S71", DisciplinaID = 2, ProfessorID = 2 },
        new Turma() { ID = 4, Codigo = "S71", DisciplinaID = 3, ProfessorID = 3 },
        new Turma() { ID = 5, Codigo = "S71", DisciplinaID = 4, ProfessorID = 5 },
        new Turma() { ID = 6, Codigo = "S71", DisciplinaID = 5, ProfessorID = 5 },
        new Turma() { ID = 7, Codigo = "S71", DisciplinaID = 6, ProfessorID = 5 },
        new Turma() { ID = 8, Codigo = "S71", DisciplinaID = 7, ProfessorID = 4 },
        new Turma() { ID = 9, Codigo = "S77", DisciplinaID = 7, ProfessorID = 9 },
        new Turma() { ID = 10, Codigo = "S71", DisciplinaID = 8, ProfessorID = 6 },
        new Turma() { ID = 11, Codigo = "S77", DisciplinaID = 8, ProfessorID = 10 },
        new Turma() { ID = 12, Codigo = "S71", DisciplinaID = 9, ProfessorID = 7 },
        new Turma() { ID = 13, Codigo = "S73", DisciplinaID = 9, ProfessorID = 7 },
        new Turma() { ID = 14, Codigo = "S71", DisciplinaID = 10, ProfessorID = 8 },
        new Turma() { ID = 15, Codigo = "S73", DisciplinaID = 11, ProfessorID = 4 },
        new Turma() { ID = 16, Codigo = "S73", DisciplinaID = 11, ProfessorID = 10 },
        new Turma() { ID = 17, Codigo = "S71", DisciplinaID = 12, ProfessorID = 5 }
    };

    public IEnumerable<Aluno> Alunos { get; set; } = new List<Aluno>()
    {
        new Aluno() { ID = 1, TurmasMatriculados = new List<int>() { 16, 14, 13, 10, 9, 3, 6 },
            Nome = "Leonardo Trevisan Silio", Idade = 18},
        new Aluno() { ID = 2, TurmasMatriculados = new List<int>() { 15, 14, 11, 8, 3, 6, 9 },
            Nome = "Carol Rosa", Idade = 18},
        new Aluno() { ID = 3, TurmasMatriculados = new List<int>() { 16, 14, 9, 6, 3, 7, 1, 2 },
            Nome = "Bruna Pinheirinho", Idade = 14},
        new Aluno() { ID = 4, TurmasMatriculados = new List<int>() { 15, 12, 9, 5, 2, 7, 4 },
            Nome = "Tiago Sendeski", Idade = 21},
        new Aluno() { ID = 5, TurmasMatriculados = new List<int>() { 1, 2, 3, 4, 5, 6, 8 },
            Nome = "Ian Doublas", Idade = 23},
        new Aluno() { ID = 6, TurmasMatriculados = new List<int>() { 17, 16, 15, 14, 13, 12, 11 },
            Nome = "Moisés Dias", Idade = 24},
        new Aluno() { ID = 7, TurmasMatriculados = new List<int>() { 1, 8, 15, 7, 13, 3, 11, 4 },
            Nome = "Alan Jun Onoda", Idade = 18},
        new Aluno() { ID = 7, TurmasMatriculados = new List<int>() {4, 13, 5, 6 , 17, 11, 13, 9 },
            Nome = "Wagber", Idade = 19},
        new Aluno() { ID = 7, TurmasMatriculados = new List<int>() { 3, 2, 1, 17, 5, 4, 6, 7, 8, 14 },
            Nome = "Vitor Corra", Idade = 18},
        new Aluno() { ID = 7, TurmasMatriculados = Enumerable.Range(1, 12).ToList(),
            Nome = "Gabriel Maia", Idade = 18}
    };
}