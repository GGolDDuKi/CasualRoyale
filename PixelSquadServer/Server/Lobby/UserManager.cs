using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public class UserManager
	{
		public static UserManager Instance { get; } = new UserManager();

		object _lock = new object();
		Dictionary<int, User> _users = new Dictionary<int, User>();
		public Dictionary<int, User> Users { get { return _users; } }

		// [UNUSED(1)][TYPE(7)][ID(24)]
		int _counter = 0;

		public T Add<T>() where T : User, new()
		{
			T user = new T();

			lock (_lock)
			{
				user.Id = GenerateId(GameObjectType.Player);
				_users.Add(user.Id, user as User);
			}

			return user;
		}

		int GenerateId(GameObjectType type)
		{
			lock (_lock)
			{
				return ((int)type << 24) | (_counter++);
			}
		}

		public static GameObjectType GetObjectTypeById(int id)
		{
			int type = (id >> 24) & 0x7F;
			return (GameObjectType)type;
		}

		public bool Remove(int objectId)
		{
			GameObjectType objectType = GetObjectTypeById(objectId);

			lock (_lock)
			{
				if (objectType == GameObjectType.Player)
					return _users.Remove(objectId);
			}

			return false;
		}

		public User Find(int objectId)
		{
			GameObjectType objectType = GetObjectTypeById(objectId);

			lock (_lock)
			{
				if (objectType == GameObjectType.Player)
				{
					User user = null;
					if (_users.TryGetValue(objectId, out user))
						return user;
				}
			}

			return null;
		}
	}
}
