using Library.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IRepository
{
    public interface IMemberRepository
    {
        Member Add(bool isStudent, decimal initialBalance = 0.0m);
        Member? GetById(int id);
        IEnumerable<Member> GetAll();
        bool Remove(int id);
    }

    public class InMemoryMemberRepository : IMemberRepository
    {
        private readonly ConcurrentDictionary<int, Member> _members = new();
        private int _nextId = 1;

        public Member Add(bool isStudent, decimal initialBalance = 0.0m)
        {
            int id = Interlocked.Increment(ref _nextId);
            Member member = new Member(id, isStudent, initialBalance);
            _members.TryAdd(id, member);
            return member;
        }

        public Member? GetById(int id) => _members.TryGetValue(id, out var member) ? member : null;

        public IEnumerable<Member> GetAll() => _members.Values;

        public bool Remove(int id) => _members.TryRemove(id, out _);
    }
}
