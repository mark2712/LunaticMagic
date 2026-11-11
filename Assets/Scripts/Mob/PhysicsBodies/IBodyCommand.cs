namespace Mobs.Physics
{
    public interface IBodyCommand
    {
        void ExecuteCommand(IPhysicsBody body);
    }
}