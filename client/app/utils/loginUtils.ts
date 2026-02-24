export const generateUsername = (firstName: string, lastName: string) => {
  return (
    firstName.trim() +
    lastName.toLowerCase().trim() +
    Math.floor(Math.random() * 99) + 1
  );
};