using FluentValidation;

namespace KanbanBoard2.Validators
{
    public class StoryValidator : AbstractValidator<Story>
    {
        public StoryValidator()
        {
            RuleFor(story => story.Name).NotEmpty();
        }
    }
}