import sqlite3

# Function to create the factions table
def create_table(cursor):
    cursor.execute('''CREATE TABLE IF NOT EXISTS factions (
                        id INTEGER PRIMARY KEY,
                        name TEXT
                    )''')

# Function to insert factions into the database
def insert_factions(cursor, factions):
    for faction in factions:
        cursor.execute('''INSERT INTO factions (name) VALUES (?)''', (faction,))

# Main function
def main():
    # Open the connection to the SQLite database
    conn = sqlite3.connect('factions.db')
    cursor = conn.cursor()

    # Create the factions table if it doesn't exist
    create_table(cursor)

    # Read the list of factions from the text file
    with open('factions.txt', 'r') as file:
        factions = [line.strip() for line in file]

    # Insert the factions into the database
    insert_factions(cursor, factions)

    # Commit changes and close the connection
    conn.commit()
    conn.close()

if __name__ == "__main__":
    main()
