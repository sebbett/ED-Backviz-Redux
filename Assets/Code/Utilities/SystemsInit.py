import sqlite3

# Function to create the systems table
def create_table(cursor):
    cursor.execute('''CREATE TABLE IF NOT EXISTS systems (
                        id INTEGER PRIMARY KEY,
                        name TEXT
                    )''')

# Function to insert systems into the database
def insert_systems(cursor, systems):
    for system in systems:
        cursor.execute('''INSERT INTO systems (name) VALUES (?)''', (system,))

# Main function
def main():
    # Open the connection to the SQLite database
    conn = sqlite3.connect('systems.db')
    cursor = conn.cursor()

    # Create the systems table if it doesn't exist
    create_table(cursor)

    # Read the list of systems from the text file
    with open('Systems.txt', 'r') as file:
        systems = [line.strip() for line in file]

    # Insert the systems into the database
    insert_systems(cursor, systems)

    # Commit changes and close the connection
    conn.commit()
    conn.close()

if __name__ == "__main__":
    main()
